export const SIZE_CHANGED = 'space/SIZE_CHANGED'
export const VOXEL_CHANGED = 'space/VOXEL_CHANGED'
export const VOXEL_CHANGED_BATCH = 'space/VOXEL_CHANGED_BATCH'
export const BOT_CHANGED = 'space/BOT_CHANGED'
export const COLOR_CHANGED = 'space/COLOR_CHANGED'
export const ENERGY_CHANGED = 'space/ENERGY_CHANGED'
export const HARMONIC_CHANGED = 'space/HARMONIC_CHANGED'
export const MESSAGE_CHANGED = 'space/MESSAGE_CHANGED'
export const COLOR_CHANGED_BATCH = 'space/COLOR_CHANGED_BATCH'

const initialState = {
  size: 30,
  voxels: {},
  colors: {},
  bots: {},
  energy: 0,
  harmonic: false,
  message: ''
}

export default (state = initialState, action) => {
  switch (action.type) {
    case SIZE_CHANGED:
      return {
        ...state,
        size: action.payload.size
      }
    case ENERGY_CHANGED:
      return {
        ...state,
        energy: action.payload
      }
    case MESSAGE_CHANGED:
      return {
        ...state,
        message: action.payload
      }
    case HARMONIC_CHANGED:
      return {
        ...state,
        harmonic: action.payload
      }
    case VOXEL_CHANGED:
      return {
        ...state,
        voxels: {
          ...state.voxels,
          [action.payload.position]: action.payload.filled
        }
      }
    case VOXEL_CHANGED_BATCH: {
      let newVoxels = {}
      for (let pos of action.payload.positions) {
        newVoxels[pos] = action.payload.value
      }
      return {
        ...state,
        voxels: {
          ...state.voxels,
          ...newVoxels
        }
      }
    }
    case COLOR_CHANGED_BATCH: {
      const { color, opacity, positions } = action.payload
      let deleted = color === '000000'
      let colorOpKey = `0x${color}/${opacity}`
      let posForKey = state.colors[colorOpKey] || {}
      let oldKeys = []
      for (let p of positions) {
        posForKey[p] = !deleted
        for (let key of Object.keys(state.colors)) {
          if (key === colorOpKey)
            continue
          if (state.colors[key][p]) {
            oldKeys[p] = key
          }
        }
      }

      let newColors = {
        ...state.colors,
        [colorOpKey]: posForKey
      }
      for (let [pos, key] of Object.entries(oldKeys)) {
        newColors[key] = {
          ...newColors[key],
          [pos]: false
        }
      }

      return {
        ...state,
        colors: newColors
      }
    }
    case COLOR_CHANGED: {
      const { color, opacity, position } = action.payload
      let colorOpKey = `0x${color}/${opacity}`
      let positionsForKey = state.colors[colorOpKey] || {}
      let deleted = color === '000000'
      let oldKey
      for (let key of Object.keys(state.colors)) {
        if (key === colorOpKey)
          continue
        if (state.colors[key][position]) {
          oldKey = key
        }
      }

      const oldKeyExpand = oldKey ? {
        [oldKey]: {
          ...state.colors[oldKey],
          [position]: false
        }
      } : {}

      let newPositionsForKey = {
        ...positionsForKey,
        [position]: !deleted
      }
      return {
        ...state,
        colors: {
          ...state.colors,
          ...oldKeyExpand,
          [colorOpKey]: newPositionsForKey
        }
      }
    }
    case BOT_CHANGED:
      return {
        ...state,
        bots: {
          ...state.bots,
          [action.payload.position]: action.payload.exists
        }
      }
    case 'RESET':
      return initialState
    default:
      return state
  }
}

export const changeSize = (newSize) => {
  return dispatch => {
    dispatch({
      type: SIZE_CHANGED,
      payload: {
        size: newSize
      }
    })
  }
}

export const changeHarmonic = (newHar) => {
  return dispatch => {
    dispatch({
      type: HARMONIC_CHANGED,
      payload: newHar
    })
  }
}

export const changeEnergy = (newEn) => {
  return dispatch => {
    dispatch({
      type: ENERGY_CHANGED,
      payload: newEn
    })
  }
}

export const changeMessage = (newM) => {
  return dispatch => {
    dispatch({
      type: MESSAGE_CHANGED,
      payload: newM
    })
  }
}

export const changeColor = (position, color, opacity = 0.5) => {
  return dispatch => {
    dispatch({
      type: COLOR_CHANGED,
      payload: {
        position,
        color,
        opacity
      }
    })
  }
}

export const changeVoxelBatch = (positions, value) => {
  return dispatch => {
    dispatch({
      type: VOXEL_CHANGED_BATCH,
      payload: {
        positions,
        value
      }
    })
  }
}

export const changeColorBatch = (positions, color, opacity) => {
  return dispatch => {
    dispatch({
      type: COLOR_CHANGED_BATCH,
      payload: {
        positions,
        color,
        opacity
      }
    })
  }
}

export const changeVoxel = (position, filled) => {
  return dispatch => {
    dispatch({
      type: VOXEL_CHANGED,
      payload: {
        filled,
        position: position.serialize ? position.serialize() : position
      }
    })
  }
}

export const changeBot = (position, exists) => {
  return dispatch => {
    dispatch({
      type: BOT_CHANGED,
      payload: {
        exists,
        position: position.serialize ? position.serialize() : position
      }
    })
  }
}
