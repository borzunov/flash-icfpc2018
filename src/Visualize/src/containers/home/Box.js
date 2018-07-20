import React from 'react'
import * as THREE from 'three'
import { connect } from 'react-redux'
import {compose, pure} from 'recompose';

const BoxImpl = ({ position, color, size, filled = false, contoured = false }) => {
  const box = new THREE.BoxBufferGeometry(size.x, size.y, size.z)
  if (!filled && !contoured) {
    return null;

  }
  else if (contoured){
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
        color={color}
      />
    </mesh>
  )
}

const Box = compose(connect(
  (({ space }, {posKey, contoured}) => {
    let filled = space.voxels[posKey]
    let color;
    if (filled)
      color = 0xffffff;
    else if (contoured)
      color = 0x00ff00;
    return {
      filled,
      color,
    }
  })
), pure)(BoxImpl)

export default Box