import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import { dataStore } from '../../store'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'
import Box from './Box'

const ColorContainerImpl = ({ boxSize, colors }) => {
  return (<group>
    {Object.entries(colors)
      .filter(([pos, {color}]) => color && color !== '000000')
      .map(([pos, {color, opacity}]) => {
        return (
          <Box store={dataStore} color={Number(`0x${color}`)} size={boxSize} position={vecToThree(deserializeVector(pos), boxSize)}
                   filled={true} opacity={opacity} key={pos} posKey={pos}/>)
      })
    }</group>)
}

const ColorContainer = compose(
  withProps({ store: dataStore }),
  connect(({ space }) => ({ colors: space.colors })),
  pure
)(ColorContainerImpl)

export default ColorContainer