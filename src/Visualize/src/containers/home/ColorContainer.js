import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import { dataStore } from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import { BoxImpl } from './Box'

const ColorContainerImpl = ({ boxSize, colors }) => {
  return (<group>
    {Object.entries(colors)
      .filter(([pos, color]) => color && color !== '000000' && color !== '000')
      .map(([pos, color]) => {
        return (
          <BoxImpl color={Number(`0x${color}`)} size={boxSize} position={vecToThree(deserializeVector(pos), boxSize)}
                   filled={true} transparent key={pos} posKey={pos}/>)
      })
    }</group>)
}

const ColorContainer = compose(
  withProps({ store: dataStore }),
  connect(({ space }) => ({ colors: space.colors })),
  pure
)(ColorContainerImpl)

export default ColorContainer