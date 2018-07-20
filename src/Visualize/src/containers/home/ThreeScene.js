import React from 'react'
import * as THREE from 'three'
import BoxImpl from './Box'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'

// noinspection JSUnresolvedFunction
const OrbitControls = require('three-orbit-controls')(THREE)


export default class ThreeScene extends React.Component {
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
    return Number(parseInt(Math.random() * max % max));
  }

  render() {
    const width = window.innerWidth // canvas width
    const height = window.innerHeight // canvas height
    const { size, changeSize, changeVoxel } = this.props

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(size, size, size)
    return (<div>
      <div style={{ position: 'absolute', left: 0, top: 0, color: 'red' }}>
        <div>Left mouse - rotate camera</div>
        <div>Mouse scroll - zoom</div>
        <div>Right mouse - pan</div>
        <button onClick={() => changeSize(this.makeRand(10))}>change size random</button>
        <button onClick={() => {
          const rand = Vector(this.makeRand(size), this.makeRand(size), this.makeRand(size))
          return changeVoxel(rand, true)
        }}>fill random voxel
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
          <BoxImpl color={0xffffff} position={new THREE.Vector3(0, 0, 0)} size={bigBoxSize}/>
          <BoxImpl color={0x00ff00} position={new THREE.Vector3(0, 0, 0)} size={smallBoxSize} solid/>
          <BoxImpl color={0xff0000} position={new THREE.Vector3(1, 0, 0)} size={smallBoxSize}/>
          <BoxImpl color={0x00ff00} position={new THREE.Vector3(2, 0, 0)} size={smallBoxSize}/>
          <BoxImpl color={0xff0000} position={new THREE.Vector3(3, 0, 0)} size={smallBoxSize}/>
          <BoxImpl color={0x00ff00} position={new THREE.Vector3(4, 0, 0)} size={smallBoxSize}/>
          <BoxImpl color={0xff0000} position={new THREE.Vector3(5, 0, 0)} size={smallBoxSize}/>
        </scene>
      </React3>
    </div>)
  }
}