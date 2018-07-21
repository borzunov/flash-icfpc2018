import React from 'react'
import { compose, pure, withProps } from 'recompose'
import { connect } from 'react-redux'
import {dataStore} from '../../store'
import { BoxImpl } from './Box'
import { vecToThree } from './coords'
import { deserializeVector } from '../../modules/Vector'

const BotContainerImpl = ({ bots, botSize, botColor }) => {
  return (<group>
    {Object.entries(bots)
      .filter(([pos, exists]) => exists)
      .map(([pos]) => {
        return (<BoxImpl key={pos} position={vecToThree(deserializeVector(pos), botSize)} color={botColor} filled={true}
                         size={botSize}/>)
      })
    }</group>)
}

const BotContainer = compose(withProps({ store: dataStore }),
  connect(
    ({ space }) => ({ bots: space.bots })
  ), pure)(BotContainerImpl)

export default BotContainer