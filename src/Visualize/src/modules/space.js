export const SIZE_CHANGED = 'space/SIZE_CHANGED'
export const VOXEL_CHANGED = 'space/VOXEL_CHANGED'
export const BOT_CHANGED = 'space/BOT_CHANGED'
export const COLOR_CHANGED = 'space/COLOR_CHANGED'

const initialState = {
  size: 30,
  voxels: {},
  colors: {},
  bots: {}
}

export default (state = initialState, action) => {
  switch (action.type) {
    case SIZE_CHANGED:
      return {
        ...state,
        size: action.payload.size
      }
    case VOXEL_CHANGED:
      return {
        ...state,
        voxels: {
          ...state.voxels,
          [action.payload.position]: action.payload.filled
        }
      }
    case COLOR_CHANGED:
      return {
        ...state,
        colors: {
          ...state.colors,
          [action.payload.position]: action.payload.color
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

export const changeColor = (position, color) => {
  return dispatch => {
    dispatch({
      type: COLOR_CHANGED,
      payload: {
        position,
        color
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