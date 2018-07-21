import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import {dataStore} from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import Box from './Box'

const FillContainerImpl = ({ boxSize, voxels }) => {
  return (<group>
    {Object.entries(voxels)
      .filter(([pos, exists]) => exists)
      .map(([pos]) => {
        return (<Box store={dataStore} size={boxSize} position={vecToThree(deserializeVector(pos), boxSize)} key={pos}
                     showEdges posKey={pos}/>)
      })
    }</group>)
}

const FillContainer = compose(
  withProps({store: dataStore}),
  connect(({ space }) => ({ voxels: space.voxels })),
  pure
)(FillContainerImpl)

export default FillContainer