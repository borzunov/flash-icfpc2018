import React from 'react'
import * as THREE from 'three'
import Box from './Box'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'
import store from '../../store'

const vecToThree = (vec, size) => {
  return new THREE.Vector3(vec.x + size.x / 2, vec.y + size.y / 2, vec.z + size.z / 2)
}

// noinspection JSUnresolvedFunction
const OrbitControls = require('three-orbit-controls')(THREE)


export default class ThreeScene extends React.PureComponent {
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
    const { size } = this.props
    const rand = Vector(this.makeRand(size), this.makeRand(size), this.makeRand(size))
    return this.props.changeVoxel(rand, true)
  }

  render() {
    const width = window.innerWidth // canvas width
    const height = window.innerHeight // canvas height
    const { size, changeSize } = this.props

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(size, size, size)
    const boxes = []

    for (let i = 0; i < size; ++i) {
      for (let j = 0; j < size; ++j) {
        for (let k = 0; k < size; ++k) {
          const pos = Vector(i, j, k)
          const threePos = vecToThree(pos, smallBoxSize)
          const key = pos.serialize()
          boxes.push(<Box store={store} color={0x00ff00} size={smallBoxSize} position={threePos} key={key}
                          posKey={key}/>)
        }
      }
    }


    return (<div>
      <div style={{ position: 'absolute', left: 0, top: 0, color: 'red' }}>
        <div>Open in chrome and install <a target="_blank" rel="noopener noreferrer"
                                           href="https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd?hl=en">redux
          dev tools</a></div>
        <div>Left mouse - rotate camera</div>
        <div>Mouse scroll - zoom</div>
        <div>Right mouse - pan</div>
        <div>Arrow buttons - move camera</div>
        <button onClick={() => changeSize(this.makeRand(10))}>change size random</button>
        <button onClick={this.fillRandomVoxel}>fill random voxel
        </button>
        <button onClick={() => {
          for (let i = 0; i < size * size * size / 8; ++i) {
            setTimeout(() => this.fillRandomVoxel(), 10)
          }
        }}>fucking overload 1/8
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
            position={vecToThree(Vector(size * 0.75, size * 0.75, size * 2.5), bigBoxSize)}
          />
          <Box store={store} color={0xffffff} position={vecToThree(Vector(0, 0, 0), bigBoxSize)} size={bigBoxSize}
               contoured/>
          {boxes}
        </scene>
      </React3>
    </div>)
  }
}