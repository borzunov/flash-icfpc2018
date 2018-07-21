import React from 'react'
import * as THREE from 'three'
import { connect } from 'react-redux'
import { compose, pure, withProps } from 'recompose'
import { dataStore } from '../../store'

export const BoxImpl = ({ position, color, size, filled = false, contoured = false, transparent = false }) => {
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

  return (<mesh
      position={position}
    >
      <boxGeometry
        width={size.x}
        height={size.y}
        depth={size.z}
      />
      <meshBasicMaterial
        //wireframe={true}
        transparent={transparent}
        opacity={0.5}
        color={color}
      />
    </mesh>
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
  ), pure)(BoxImpl)

export default Box