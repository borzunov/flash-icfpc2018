import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import { dataStore } from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import Box from './Box'
import Vector from '../../modules/Vector'
import * as THREE from 'three'

const shifts = [
  Vector(1, 0, 0),
  Vector(-1, 0, 0),
  Vector(0, 1, 0),
  Vector(0, -1, 0),
  Vector(0, 0, 1),
  Vector(0, 0, -1)
]
const shift = (serVec) => {
  const vec = deserializeVector(serVec)
  const shifted = []
  for (let s of shifts) {
    shifted.push(vec.add(s))
  }
  return shifted.map(v => v.serialize())
}

class MergedFillContainer extends React.PureComponent {
  render() {
    const { boxSize, voxels, color, colormap } = this.props
    const boxGeo = new THREE.BoxGeometry(boxSize.x, boxSize.y, boxSize.z)

    if (!colormap) {
      const rootGeo = new THREE.Geometry()
      for (let [pos, exists] of Object.entries(voxels)) {
        if (!exists)
          continue
        let vec = deserializeVector(pos)
        let newX = vec.x + boxSize.x / 2
        let newY = vec.y + boxSize.y / 2
        let newZ = vec.z + boxSize.z / 2
        boxGeo.translate(newX, newY, newZ)
        rootGeo.merge(boxGeo)
        boxGeo.translate(-newX, -newY, -newZ)
      }
      return <group>
        <lineSegments>
          <edgesGeometry geometry={rootGeo}/>
          <lineBasicMaterial color={0} linewidth={2}/>
        </lineSegments>
        <mesh>
          <geometry vertices={rootGeo.vertices} faces={rootGeo.faces}/>
          <meshBasicMaterial color={color} vertexColors={THREE.VertexColors}/>
        </mesh>
      </group>
    }

    // colormap mode
    let objs = []
    let i = 0
    for (let [colorOpacity, positions] of Object.entries(voxels)) {
      const [color, opacity] = colorOpacity.split('/').map(Number);
      const rootGeo = new THREE.Geometry()
      for (let [pos, exists] of Object.entries(positions)) {
        if (!exists)
          continue;
        let vec = deserializeVector(pos)
        let newX = vec.x + boxSize.x / 2
        let newY = vec.y + boxSize.y / 2
        let newZ = vec.z + boxSize.z / 2
        boxGeo.translate(newX, newY, newZ)
        rootGeo.merge(boxGeo)
        boxGeo.translate(-newX, -newY, -newZ)
      }
      objs.push(<mesh key={i}>
        <geometry vertices={rootGeo.vertices} faces={rootGeo.faces}/>
        <meshBasicMaterial color={color} transparent={opacity < 1} opacity={opacity} vertexColors={THREE.VertexColors}/>
      </mesh>)
      i++;
    }
    return <group>
      {objs}
    </group>
  }
}

const FillContainerImpl = ({ boxSize, voxels, useMerge, color, colormap }) => {
  if (useMerge)
    return <MergedFillContainer boxSize={boxSize} voxels={voxels} color={color} colormap={colormap}/>
  let invisibleMap = {}
  return (<group>
    {Object.entries(voxels)
      .filter(([pos, exists]) => exists)
      .map(([pos]) => {
        let count = 0
        let neighbours = shift(pos)
        for (let neigh of neighbours) {
          if (voxels[neigh])
            count++
        }
        if (count === 6) {
          invisibleMap[pos] = true
        }
        return [pos]
      })
      .map(([pos]) => {
        return (<Box store={dataStore} size={boxSize} position={vecToThree(deserializeVector(pos), boxSize)} key={pos}
                     invisible={invisibleMap[pos]} showEdges posKey={pos} color={0xffffff} filled/>)
      })
    }</group>)
}

const FillContainer = compose(
  withProps({ store: dataStore }),
  connect(({ space }, { target }) => ({ voxels: space[target] })),
  pure
)(FillContainerImpl)

export default FillContainer