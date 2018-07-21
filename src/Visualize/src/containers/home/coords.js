import * as THREE from 'three'

export const vecToThree = (vec, size) => {
  return new THREE.Vector3(vec.x + size.x / 2, vec.y + size.y / 2, vec.z + size.z / 2)
}