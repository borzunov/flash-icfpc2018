import React from 'react'
import * as THREE from 'three'


const BoxImpl = ({ position, color, size, solid = false}) => {
  const box = new THREE.BoxBufferGeometry(size.x, size.y, size.z)
  if (!solid)
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

export default BoxImpl;