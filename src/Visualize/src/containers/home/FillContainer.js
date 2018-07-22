import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import { dataStore } from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import Box from './Box'
import Vector from '../../modules/Vector'

const shifts = [
  Vector(1, 0, 0),
  Vector(-1, 0, 0),
  Vector(0, 1, 0),
  Vector(0, -1, 0),
  Vector(0, 0, 1),
  Vector(0, 0, -1),
]
const shift = (serVec) => {
  const vec = deserializeVector(serVec)
  const shifted = []
  for (let s of shifts) {
    shifted.push(vec.add(s))
  }
  return shifted.map(v => v.serialize())
}

const FillContainerImpl = ({ boxSize, voxels }) => {
  let invisibleMap = {}
  return (<group>
    {Object.entries(voxels)
      .filter(([pos, exists]) => exists)
      .map(([pos]) => {
        let count = 0;
        let neighbours = shift(pos);
        for (let neigh of neighbours) {
          if (voxels[neigh])
            count++;
        }
        if (count === 6) {
          invisibleMap[pos] = true;
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
  connect(({ space }) => ({ voxels: space.voxels })),
  pure
)(FillContainerImpl)

export default FillContainer