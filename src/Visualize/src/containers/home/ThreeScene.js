import React from 'react'
import * as THREE from 'three'
import React3 from 'react-three-renderer'
import Vector from '../../modules/Vector'
import BotContainer from './BotContainer'
import { vecToThree } from './coords'
import { withSize } from 'react-sizeme'
import CoordinatesHelpers from './CoordinatesHelpers'
import FillContainer from './FillContainer'
import { slide as Menu } from 'react-burger-menu'
import HelpText from './HelpText'
import LogPlayer from './LogPlayer'
import ColorContainer from './ColorContainer'
import InfoContainer from './InfoContainer'
import { Tab, Tabs, TabList, TabPanel } from 'react-tabs'
import 'react-tabs/style/react-tabs.css'

// noinspection JSUnresolvedFunction
const OrbitControls = require('three-orbit-controls')(THREE)
const smallBoxSize = new THREE.Vector3(1, 1, 1)
const botSize = new THREE.Vector3(0.75, 0.75, 0.75)

function makeCameraPosition(mapSize, bigBoxSize) {
  return vecToThree(Vector(mapSize * 1, mapSize * 1, mapSize * 1.5), bigBoxSize)
}

class ThreeScene extends React.PureComponent {
  state = {
    menuOpen: true
  }

  componentDidMount() {
    window.controls = this.controls = new OrbitControls(this.camera)
    this.updateCamera()
    this.bindKeys(true)
  }

  bindKeys(enable) {
    if (enable) {
      this.keyEventHandler = (e) => {
        if (e.keyCode === 77) // m
          this.setState({ menuOpen: !this.state.menuOpen })
      }
      document.addEventListener('keydown', this.keyEventHandler)
    }
    else {
      document.removeEventListener('keydown', this.keyEventHandler)
    }
  }

  updateCamera = () => {
    let mapSize = this.props.mapSize
    const ms2 = mapSize / 2
    this.controls.reset()
    let target = new THREE.Vector3(ms2, ms2, ms2)
    this.controls.target = target
    this.camera.lookAt(target)
  }

  componentDidUpdate() {
    this.updateCamera()
  }

  componentWillUnmount() {
    this.controls.dispose()
    this.controls = null
    this.bindKeys(true)
  }

  render() {
    const { mapSize } = this.props
    let { width, height } = this.props.size
    width = this.state.menuOpen ? width - 300 : width
    const bigBoxSize = new THREE.Vector3(mapSize, mapSize, mapSize)

    return (<div style={{ width: '100%', height: '100%' }}>
      <Menu isOpen={this.state.menuOpen} noOverlay closeButton={false} width={400}
            onStateChange={({ isOpen }) => this.setState({ menuOpen: isOpen })}
            styles={{
              bmMenuWrap: {
                transition: '0s'
              }
            }}
      >
        <div style={{ overflow: 'hidden', height: '100%', width: '100%', paddingTop: 20, position: 'relative' }}>
          <Tabs defaultIndex={0} className="react-tabs-main">
            <TabList>
              <Tab>Log player</Tab>
              <Tab>Model viewer</Tab>
            </TabList>
            <TabPanel style={{ position: 'relative', width: '100%' }}><LogPlayer bd="logs" limit={10}/></TabPanel>
            <TabPanel style={{ position: 'relative', width: '100%' }}><LogPlayer bd="models"/></TabPanel>
          </Tabs></div>
      </Menu>
      <div style={{
        paddingLeft: this.state.menuOpen ? 300 : 0,
        overflow: 'hidden',
        backgroundColor: 'black'
      }}>
        <HelpText/>
        <InfoContainer/>
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
              position={makeCameraPosition(mapSize, bigBoxSize)}
            />
            <CoordinatesHelpers mapSize={mapSize} bigBoxSize={bigBoxSize}/>
            <FillContainer boxSize={smallBoxSize}/>
            <ColorContainer boxSize={smallBoxSize}/>
            <BotContainer botSize={botSize} botColor={0xffa500}/>
          </scene>
        </React3></div>
    </div>)
  }
}

export default withSize({ monitorHeight: true, refreshRate: 500 })(ThreeScene)