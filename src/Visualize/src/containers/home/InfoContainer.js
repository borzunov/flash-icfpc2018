import React from 'react'
import { compose, pure, withProps } from 'recompose'

import { connect } from 'react-redux'
import { dataStore } from '../../store'

const ColorContainerImpl = ({ energy, harmonic, message }) => {
  return (<div style={{position: 'absolute', bottom: 0, right: 0, textAlign: 'right', color: 'white', padding: 20}}>
      <div>Energy: {energy}</div>
      <div>Harmonic: {harmonic ? 'high' : 'low'}</div>
      <div>Message: {message}</div>
  </div>)
}

const ColorContainer = compose(
  withProps({ store: dataStore }),
  connect(({ space }) => ({ energy: space.energy, harmonic: space.harmonic, message: space.message })),
  pure
)(ColorContainerImpl)

export default ColorContainer