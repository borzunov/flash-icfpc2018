import React from 'react'
import * as THREE from 'three'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'
import store from '../../store'
import BotContainer from './BotContainer'
import { vecToThree } from './coords'
import { withSize } from 'react-sizeme'
import CoordinatesHelpers from './CoordinatesHelpers'
import ControlPanel from './ControlPanel'
import { LogAction } from '../../test-logs/LogAction'
import FillContainer from './FillContainer'
import Queue from 'async/queue'
import { slide as Menu } from 'react-burger-menu'
import HelpText from './HelpText'

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
      <Menu isOpen={true} noOverlay closeButton={false}>
        <ControlPanel
          {...
            {
              changeSize,
              mapSize,
              fillRandomVoxel: this.fillRandomVoxel,
              addRandomBot: this.addRandomBot,
              doPlayLog: (testCase) => playLog(this.props, testCase),
              doReset: reset,
              enqueue: (task, cb) => syncQueueWithWait.push(task, cb)
            }
          }
        />
      </Menu>
      <HelpText/>
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
          <CoordinatesHelpers mapSize={mapSize} bigBoxSize={bigBoxSize}/>
          <FillContainer boxSize={smallBoxSize}/>
          <BotContainer botSize={botSize} botColor={0xffa500}/>
        </scene>
      </React3>
    </div>)
  }
}

export default withSize({ monitorHeight: true, refreshRate: 500 })(ThreeScene)