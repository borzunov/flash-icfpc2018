import React from 'react'
import * as THREE from 'three'
import { connect } from 'react-redux'
import { compose, pure, shouldUpdate, withProps } from 'recompose'
import { dataStore } from '../../store'

export const BoxImpl = ({ position, color, size, filled = false, contoured = false, opacity = 1, showEdges = false }) => {
  const box = new THREE.BoxBufferGeometry(size.x, size.y, size.z)
  if (!filled && !contoured) {
    return null

  }
  else if (contoured) {
    return <lineSegments position={position}>
      <edgesGeometry geometry={box}/>
      <lineBasicMaterial color={color} linewidth={2}/>
    </lineSegments>
  }

  return (<group>
      {showEdges && <lineSegments position={position}>
        <edgesGeometry geometry={box}/>
        <lineBasicMaterial color={0} linewidth={2}/>
      </lineSegments>}
      <mesh
        position={position}
      >
        <boxGeometry
          width={size.x * 0.95}
          height={size.y * 0.95}
          depth={size.z * 0.95}
        />
        <meshBasicMaterial
          transparent={opacity < 1}
          opacity={opacity}
          color={color}
        />
      </mesh>
    </group>
  )
}

const Box = compose(
  withProps({ store: dataStore }),
  connect(
    (({ space }, { posKey, contoured, color }) => {
      let filled = space.voxels[posKey]
      if (!color) {
        if (filled)
          color = 0xffffff
        else if (contoured)
          color = 0x00ff00
      }
      return {
        filled,
        color
      }
    })
  ), shouldUpdate(() => false))
(BoxImpl)

export default Box