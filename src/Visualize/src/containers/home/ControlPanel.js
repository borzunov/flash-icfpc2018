import case1 from '../../test-logs/1'
import React from 'react'
import HelpText from './HelpText'

export default function ControlPanel({ changeSize, mapSize, fillRandomVoxel, addRandomBot, doPlayLog, doReset, enqueue }) {
  return <div>
    <HelpText/>
    <div style={{ position: 'absolute', left: 0, top: 0, color: 'white' }}>
      <button onClick={() => {
        return changeSize(Math.min(mapSize + 10, 250))
      }}>size + 10
      </button>
      <button onClick={() => {
        return changeSize(Math.max(10, mapSize - 10))
      }}>size - 10
      </button>
      <button onClick={() => {
        doReset()
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
        doReset()
        doPlayLog(case1)
      }}>play test 1
      </button>
    </div>
  </div>
}