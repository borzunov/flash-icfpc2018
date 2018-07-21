import React from 'react'
import * as THREE from 'three'
import Box from './Box'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'
import store from '../../store'
import BotContainer from './BotContainer'
import { vecToThree } from './coords'
import { withSize } from 'react-sizeme'

import case1 from '../../test-logs/1.js'
import { LogAction } from '../../test-logs/LogAction'
import FillContainer from './FillContainer'
import Queue from 'async/queue'

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

// noinspection JSUnresolvedFunction
const OrbitControls = require('three-orbit-controls')(THREE)

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

class ThreeScene extends React.PureComponent {
  componentDidMount() {
    this.controls = new OrbitControls(this.camera)
  }

  componentWillUnmount() {
    this.controls.dispose()
    this.controls = null
  }

  makeRand = (max) => {
    return Number(parseInt(String(Math.random() * max % max), 10))
  }

  fillRandomVoxel = () => {
    const { mapSize } = this.props
    const rand = Vector(this.makeRand(mapSize), this.makeRand(mapSize), this.makeRand(mapSize))
    return this.props.changeVoxel(rand, true)
  }

  addRandomBot = () => {
    const { mapSize } = this.props
    const rand = Vector(this.makeRand(mapSize), this.makeRand(mapSize), this.makeRand(mapSize))
    return this.props.changeBot(rand, true)
  }

  render() {
    const { mapSize, changeSize } = this.props
    currentWait = mapSize / SPEED_CONST
    const { width, height } = this.props.size

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(mapSize, mapSize, mapSize)
    const botSize = new THREE.Vector3(0.75, 0.75, 0.75)

    return (<div style={{ width: '100%', height: '100%' }}>
      <div style={{ position: 'absolute', left: 0, top: 0, color: 'white' }}>
        <div>Open in chrome and install <a target="_blank" rel="noopener noreferrer"
                                           href="https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd?hl=en">redux
          dev tools</a></div>
        <div>Coordinates: <span style={{ color: 'red' }}>X </span><span style={{ color: 'green' }}>Y </span><span
          style={{ color: 'blue' }}>Z </span> <span>!!!Z is inverted regardless to task</span></div>
        <div>Left mouse - rotate camera</div>
        <div>Mouse scroll - zoom</div>
        <div>Right mouse - pan</div>
        <div>Arrow buttons - move camera</div>
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
        <button onClick={this.fillRandomVoxel}>fill random voxel
        </button>
        <button onClick={() => {
          for (let i = 0; i < mapSize * mapSize * mapSize / 8; ++i) {
            syncQueueWithWait.push(() => this.fillRandomVoxel())
          }
        }}>fill 1/8
        </button>
        <button onClick={() => {
          for (let i = 0; i < 10; ++i) {
            syncQueueWithWait.push(() => this.addRandomBot())
          }
        }}>add some bots
        </button>
        <button onClick={() => {
          reset()
          playLog(this.props, case1)
        }}>play test 1
        </button>
      </div>
      <React3
        mainCamera="camera" // this points to the perspectiveCamera which has the name set to "camera" below
        width={width}
        height={height}
      >
        <scene ref={(ref) => this.scene = ref} rotation={new THREE.Euler(0, 0, 0, true)}>
          <perspectiveCamera
            ref={(ref) => this.camera = ref}
            name="camera"
            fov={75}
            aspect={width / height}
            near={0.1}
            far={1000}
            position={vecToThree(Vector(mapSize * 0.75, mapSize * 0.75, mapSize * 2.5), bigBoxSize)}
          />
          <axisHelper size={mapSize + 10} position={new THREE.Vector3(0, 0, 0)}/>
          <gridHelper size={mapSize} position={vecToThree(Vector(0, -mapSize * 0.5, 0), bigBoxSize)} step={mapSize}/>
          <gridHelper size={mapSize} position={vecToThree(Vector(0, 0, -mapSize * 0.5), bigBoxSize)} step={mapSize}
                      rotation={new THREE.Euler(Math.PI / 2, 0, 0)}/>
          <gridHelper size={mapSize} position={vecToThree(Vector(-mapSize * 0.5, 0, 0), bigBoxSize)} step={mapSize}
                      rotation={new THREE.Euler(0, 0, Math.PI / 2)}/>
          <Box store={store} color={0xffffff} position={vecToThree(Vector(0, 0, 0), bigBoxSize)} size={bigBoxSize}
               contoured/>
          <FillContainer boxSize={smallBoxSize}/>
          <BotContainer botSize={botSize} botColor={0xffa500}/>
        </scene>
      </React3>
    </div>)
  }
}

export default withSize({ monitorHeight: true, refreshRate: 500 })(ThreeScene)