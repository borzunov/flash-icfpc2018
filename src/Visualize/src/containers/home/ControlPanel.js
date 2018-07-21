import case1 from '../../test-logs/1'
import React from 'react'
import { dataStore } from '../../store'
import { withHandlers, withProps } from 'recompose'
import { bindActionCreators, compose } from 'redux'
import { connect } from 'react-redux'
import { changeBot, changeColor, changeSize, changeVoxel } from '../../modules/space'
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

function ControlPanelImpl({ changeSize, mapSize, fillRandomVoxel, addRandomBot, doPlayLog }) {
  currentWait = mapSize / SPEED_CONST
  return <div className="control-panel">
    <h3>Debug controls</h3>
    <button onClick={() => {
      reset()
    }}>reset
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
          changeBot,
          changeColor,
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
    doPlayLog: ({changeSize, changeVoxel, changeBot, changeColor}) => ({size, log}) => {
      playLog({ changeSize, changeVoxel, changeBot, changeColor }, { size, log }, syncQueueWithWait.push);
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