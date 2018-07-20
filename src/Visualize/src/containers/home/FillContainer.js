import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import store from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import Box from './Box'

const FillContainerImpl = ({ boxSize, voxels }) => {
  return (<group>
    {Object.entries(voxels)
      .filter(([pos, exists]) => exists)
      .map(([pos]) => {
        return (<Box store={store} size={boxSize} position={vecToThree(deserializeVector(pos), boxSize)} key={pos}
                     posKey={pos}/>)
      })
    }</group>)
}

const FillContainer = compose(
  withProps({store}),
  connect(({ space }) => ({ voxels: space.voxels })),
  pure
)(FillContainerImpl)

export default FillContainer