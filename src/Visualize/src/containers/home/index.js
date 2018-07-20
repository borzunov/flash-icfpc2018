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

const BoxImpl = ({ position }) => {
  // var geo = new THREE.EdgesGeometry( geometry ); // or WireframeGeometry( geometry )
  //
  // var mat = new THREE.LineBasicMaterial( { color: 0xffffff, linewidth: 2 } );
  //
  // var wireframe = new THREE.LineSegments( geo, mat );
  const box = new THREE.BoxBufferGeometry(1, 1, 1);
  return <lineSegments position={position}>
    <edgesGeometry geometry={box}/>
    <lineBasicMaterial color={0x00ff00} linewidth={2}/>
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
    this.cameraPosition = new THREE.Vector3(1, 1, 5)

    this.state = {
      cubeRotation: new THREE.Euler()
    }
  }

  render() {
    const width = window.innerWidth // canvas width
    const height = window.innerHeight // canvas height

    return (<React3
      mainCamera="camera" // this points to the perspectiveCamera which has the name set to "camera" below
      width={width}
      height={height}
    >
      <scene>
        <perspectiveCamera
          name="camera"
          fov={75}
          aspect={width / height}
          near={0.1}
          far={1000}
          position={this.cameraPosition}
        />
        <BoxImpl position={new THREE.Vector3(0, 0, 0)}/>
        <BoxImpl position={new THREE.Vector3(1, 0, 0)}/>
        <BoxImpl position={new THREE.Vector3(2, 0, 0)}/>
        <BoxImpl position={new THREE.Vector3(3, 0, 0)}/>
        <BoxImpl position={new THREE.Vector3(4, 0, 0)}/>
      </scene>
    </React3>)
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
