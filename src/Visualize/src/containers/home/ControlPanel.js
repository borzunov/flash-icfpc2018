import case1 from '../../test-logs/1'
import React from 'react'
import { dataStore } from '../../store'
import { withHandlers, withProps } from 'recompose'
import { bindActionCreators, compose } from 'redux'
import { connect } from 'react-redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import Vector from '../../modules/Vector'
import Queue from 'async/queue'
import { playLog } from './playLog'

let currentWait = 10
const SPEED_CONST = 4

const syncQueueWithWait = new Queue((f, callback) => {
  f()
  setTimeout(callback, currentWait)
}, 1)


const reset = () => {
  // clear queue to avoid more actions that are not finished
  syncQueueWithWait.pause()
  syncQueueWithWait.remove(() => true)
  syncQueueWithWait.resume()
  // reset dev-tools
  if (dataStore.liftedStore)
    dataStore.liftedStore.dispatch({ type: 'RESET' })
  // reset our state
  dataStore.dispatch({ type: 'RESET' })
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
  withProps({store: dataStore}),
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
      playLog({ changeSize, changeVoxel, changeBot }, { size, log }, syncQueueWithWait.push);
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