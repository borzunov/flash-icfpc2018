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
  constructor(props, context) {
    super(props, context)

    // construct the position vector here, because if we use 'new' within render,
    // React will think that things have changed when they have not.
    this.cameraPosition = new THREE.Vector3(10, 10, 50)
  }

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
    const { size, changeSize, changeVoxel } = this.props

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
        <div>Left mouse - rotate camera</div>
        <div>Mouse scroll - zoom</div>
        <div>Right mouse - pan</div>
        <div>Arrow buttons - move camera</div>
        <button onClick={() => changeSize(this.makeRand(10))}>change size random</button>
        <button onClick={this.fillRandomVoxel}>fill random voxel
        </button>
        <button onClick={() => {
          for (let i = 0; i < size * size * size / 4; ++i) {
            setTimeout(() => this.fillRandomVoxel(), 10)
          }
        }}>fucking overload quarter
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
            position={this.cameraPosition}
          />
          <Box store={store} color={0xffffff} position={vecToThree(Vector(0, 0, 0), bigBoxSize)} size={bigBoxSize}/>
          {/*<Box store={store} color={0x00ff00} position={new THREE.Vector3(0, 0, 0)} size={smallBoxSize} solid/>*/}
          {/*<Box store={store} color={0xff0000} position={new THREE.Vector3(1, 0, 0)} size={smallBoxSize}/>*/}
          {/*<Box store={store} color={0x00ff00} position={new THREE.Vector3(2, 0, 0)} size={smallBoxSize}/>*/}
          {/*<Box store={store} color={0xff0000} position={new THREE.Vector3(3, 0, 0)} size={smallBoxSize}/>*/}
          {/*<Box store={store} color={0x00ff00} position={new THREE.Vector3(4, 0, 0)} size={smallBoxSize}/>*/}
          {/*<Box store={store} color={0xff0000} position={new THREE.Vector3(5, 0, 0)} size={smallBoxSize}/>*/}
          {boxes}
        </scene>
      </React3>
    </div>)
  }
}