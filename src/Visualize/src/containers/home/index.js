import React from 'react'
import { push } from 'connected-react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import {
  increment,
  incrementAsync,
  decrement,
  decrementAsync
} from '../../modules/counter'

import React3 from 'react-three-renderer'
import * as THREE from 'three'

const OrbitControls = require('three-orbit-controls')(THREE)


const BoxImpl = ({ position, color, size }) => {
  // var geo = new THREE.EdgesGeometry( geometry ); // or WireframeGeometry( geometry )
  //
  // var mat = new THREE.LineBasicMaterial( { color: 0xffffff, linewidth: 2 } );
  //
  // var wireframe = new THREE.LineSegments( geo, mat );
  const box = new THREE.BoxBufferGeometry(size.x, size.y, size.z)
  return <lineSegments position={position}>
    <edgesGeometry geometry={box}/>
    <lineBasicMaterial color={color} linewidth={2}/>
  </lineSegments>
  // return (<mesh
  //     position={position}
  //   >
  //     <boxGeometry
  //       width={1}
  //       height={1}
  //       depth={1}
  //     />
  //     <meshBasicMaterial
  //       color={0x00ff00}
  //     />
  //   </mesh>
  // )
}

class Simple extends React.Component {
  constructor(props, context) {
    super(props, context)

    // construct the position vector here, because if we use 'new' within render,
    // React will think that things have changed when they have not.
    this.cameraPosition = new THREE.Vector3(10, 10, 50)

    this.state = {
      cubeRotation: new THREE.Euler()
    }
  }

  componentDidMount() {
    this.controls = new OrbitControls(this.camera);
  }

  render() {
    const width = window.innerWidth // canvas width
    const height = window.innerHeight // canvas height

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(10, 10, 10)
    return (<div ref={(ref) => this.renderBox = ref}>
      <div style={{position: 'absolute', left: 0, top: 0, color: 'red'}}>WASD - move camera</div>
      <React3
        mainCamera="camera" // this points to the perspectiveCamera which has the name set to "camera" below
        width={width}
        height={height}
        ref={(ref)=> this.react3 = ref}
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
          <BoxImpl color={0x00ff00} position={new THREE.Vector3(0, 0, 0)} size={smallBoxSize}/>
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

const mapStateToProps = ({ counter }) => ({
  count: counter.count,
  isIncrementing: counter.isIncrementing,
  isDecrementing: counter.isDecrementing
})

const mapDispatchToProps = dispatch =>
  bindActionCreators(
    {
      increment,
      incrementAsync,
      decrement,
      decrementAsync,
      changePage: () => push('/about-us')
    },
    dispatch
  )

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(Simple)
