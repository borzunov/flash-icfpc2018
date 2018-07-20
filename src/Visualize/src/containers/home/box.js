import React from 'react'
import * as THREE from 'three'


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

export default BoxImpl;