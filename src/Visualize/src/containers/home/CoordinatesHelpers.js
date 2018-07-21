import * as THREE from 'three'
import { vecToThree } from './coords'
import Vector from '../../modules/Vector'
import store from '../../store'
import React from 'react'
import Box from './Box'

export default ({ mapSize, bigBoxSize }) => {
  return <group>
    <axisHelper size={mapSize + 10} position={new THREE.Vector3(0, 0, 0)}/>
    <gridHelper size={mapSize} position={vecToThree(Vector(0, -mapSize * 0.5, 0), bigBoxSize)} step={mapSize}/>
    <gridHelper size={mapSize} position={vecToThree(Vector(0, 0, -mapSize * 0.5), bigBoxSize)} step={mapSize}
                rotation={new THREE.Euler(Math.PI / 2, 0, 0)}/>
    <gridHelper size={mapSize} position={vecToThree(Vector(-mapSize * 0.5, 0, 0), bigBoxSize)} step={mapSize}
                rotation={new THREE.Euler(0, 0, Math.PI / 2)}/>
    <Box store={store} color={0xffffff} position={vecToThree(Vector(0, 0, 0), bigBoxSize)} size={bigBoxSize}
         contoured/>
  </group>
}