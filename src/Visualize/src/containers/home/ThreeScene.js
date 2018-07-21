import React from 'react'
import * as THREE from 'three'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'
import BotContainer from './BotContainer'
import { vecToThree } from './coords'
import { withSize } from 'react-sizeme'
import CoordinatesHelpers from './CoordinatesHelpers'
import ControlPanel from './ControlPanel'
import FillContainer from './FillContainer'
import { slide as Menu } from 'react-burger-menu'
import HelpText from './HelpText'


// noinspection JSUnresolvedFunction
const OrbitControls = require('three-orbit-controls')(THREE)

class ThreeScene extends React.PureComponent {
  componentDidMount() {
    this.controls = new OrbitControls(this.camera)
  }

  componentWillUnmount() {
    this.controls.dispose()
    this.controls = null
  }

  render() {
    const { mapSize } = this.props
    const { width, height } = this.props.size

    const smallBoxSize = new THREE.Vector3(1, 1, 1)
    const bigBoxSize = new THREE.Vector3(mapSize, mapSize, mapSize)
    const botSize = new THREE.Vector3(0.75, 0.75, 0.75)

    return (<div style={{ width: '100%', height: '100%' }}>
      <Menu isOpen={true} noOverlay closeButton={false}>
        <ControlPanel/>
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