import React from 'react'
import * as THREE from 'three'
import { connect } from 'react-redux'

const BoxImpl = ({ position, color, size, filled = false }) => {
  const box = new THREE.BoxBufferGeometry(size.x, size.y, size.z)
  if (!filled)
    return <lineSegments position={position}>
      <edgesGeometry geometry={box}/>
      <lineBasicMaterial color={color} linewidth={2}/>
    </lineSegments>
  return (<mesh
      position={position}
    >
      <boxGeometry
        width={size.x}
        height={size.y}
        depth={size.z}
      />
      <meshBasicMaterial
        color={color}
      />
    </mesh>
  )
}

const Box = connect(
  (({ space }, posKey) => {
    return {
      filled: space.voxels[posKey]
    }
  })
)(BoxImpl)

export default Box