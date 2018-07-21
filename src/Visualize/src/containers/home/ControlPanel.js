import case1 from '../../test-logs/1'
import React from 'react'
import store from '../../store'
import { withProps, withHandlers } from 'recompose'
import { compose } from 'redux'
import { connect } from 'react-redux'
import { bindActionCreators } from 'redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import Vector from '../../modules/Vector'
import Queue from 'async/queue'
import { LogAction } from '../../test-logs/LogAction'

let currentWait = 10
const syncQueueWithWait = new Queue((f, callback) => {
  f()
  setTimeout(callback, currentWait)
}, 1)

const SPEED_CONST = 4

const reset = () => {
  // clear queue to avoid more actions that are not finished
  syncQueueWithWait.pause()
  syncQueueWithWait.remove(() => true)
  syncQueueWithWait.resume()
  // reset dev-tools
  store.liftedStore.dispatch({ type: 'RESET' })
  // reset our state
  store.dispatch({ type: 'RESET' })
}

const playLog = ({ changeSize, changeVoxel, changeBot }, { size, log }) => {
  changeSize(size)
  for (let act of log) {
    switch (act.t) {
      case LogAction.Add:
        changeBot(act.p, true)
        break
      case LogAction.Remove:
        changeBot(act.p, false)
        break
      case LogAction.Fill:
        changeVoxel(act.p, true)
        break
      default:
        // do nothing
        break
    }
  }
}

function ControlPanelImpl({ changeSize, mapSize, fillRandomVoxel, addRandomBot, doPlayLog, enqueue }) {
  currentWait = mapSize / SPEED_CONST
  return <div className="control-panel">
    <h3>Debug controls</h3>
    <button onClick={() => {
      return changeSize(Math.min(mapSize + 10, 250))
    }}>size + 10
    </button>
    <button onClick={() => {
      return changeSize(Math.max(10, mapSize - 10))
    }}>size - 10
    </button>
    <button onClick={() => {
      reset()
    }}>reset
    </button>
    <button onClick={fillRandomVoxel}>fill random voxel
    </button>
    <button onClick={() => {
      for (let i = 0; i < mapSize * mapSize * mapSize / 8; ++i) {
        enqueue(() => fillRandomVoxel())
      }
    }}>fill 1/8
    </button>
    <button onClick={() => {
      for (let i = 0; i < 10; ++i) {
        enqueue(() => addRandomBot())
      }
    }}>add some bots
    </button>
    <button onClick={() => {
      reset()
      doPlayLog(case1)
    }}>play test 1
    </button>
  </div>
}

export default compose(
  withProps(store),
  connect(({ space }) => {
      return {
        mapSize: space.size
      }
    },
    dispatch =>
      bindActionCreators(
        {
          changeSize,
          changeVoxel,
          changeBot
        },
        dispatch
      )
  ),
  withHandlers({
      makeRand: () => (max) => {
        return Number(parseInt(String(Math.random() * max % max), 10))
      },
      enqueue: () => (task, cb) => syncQueueWithWait.push(task, cb),
    }
  ),
  withHandlers({
    doPlayLog: ({changeSize, changeVoxel, changeBot}) => ({size, log}) => {
      playLog({ changeSize, changeVoxel, changeBot }, { size, log });
    },
    fillRandomVoxel: ({ mapSize, makeRand, changeVoxel }) => () => {
      const rand = Vector(makeRand(mapSize), makeRand(mapSize), makeRand(mapSize))
      return changeVoxel(rand, true)
    },

    addRandomBot: ({ mapSize, makeRand, changeBot }) => () => {
      const rand = Vector(makeRand(mapSize), makeRand(mapSize), makeRand(mapSize))
      return changeBot(rand, true)
    }
  })
)(ControlPanelImpl)