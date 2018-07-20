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

const reset = () => {
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

    const { width, height } = this.props.size

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(mapSize, mapSize, mapSize)
    const botSize = new THREE.Vector3(0.75, 0.75, 0.75)

    return (<div style={{width: '100%', height: '100%'}}>
      <div style={{ position: 'absolute', left: 0, top: 0, color: 'red' }}>
        <div>Open in chrome and install <a target="_blank" rel="noopener noreferrer"
                                           href="https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd?hl=en">redux
          dev tools</a></div>
        <div>Left mouse - rotate camera</div>
        <div>Mouse scroll - zoom</div>
        <div>Right mouse - pan</div>
        <div>Arrow buttons - move camera</div>
        <button onClick={() => {
          reset()
          return changeSize(mapSize + 10)
        }}>size + 10
        </button>
        <button onClick={() => {
          reset()
          return changeSize(mapSize - 10)
        }}>size - 10
        </button>
        <button onClick={this.fillRandomVoxel}>fill random voxel
        </button>
        <button onClick={() => {
          reset()
          for (let i = 0; i < mapSize * mapSize * mapSize / 8; ++i) {
            setTimeout(() => this.fillRandomVoxel(), 10)
          }
        }}>fucking overload 1/8
        </button>
        <button onClick={() => {
          reset()
          for (let i = 0; i < 10; ++i) {
            setTimeout(() => this.addRandomBot(), 10)
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
        <scene>
          <perspectiveCamera
            ref={(ref) => this.camera = ref}
            name="camera"
            fov={75}
            aspect={width / height}
            near={0.1}
            far={1000}
            position={vecToThree(Vector(mapSize * 0.75, mapSize * 0.75, mapSize * 2.5), bigBoxSize)}
          />
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