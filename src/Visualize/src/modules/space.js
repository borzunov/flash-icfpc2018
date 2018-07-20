export const SIZE_CHANGED = 'space/SIZE_CHANGED'
export const VOXEL_CHANGED = 'space/VOXEL_CHANGED'
export const BOT_CHANGED = 'space/BOT_CHANGED'

const initialState = {
  size: 20,
  voxels: {
  },
  bots: {
  },
}

export default (state = initialState, action) => {
  switch (action.type) {
    case SIZE_CHANGED:
      return {
        ...state,
        size: action.payload.size,
      }
    case VOXEL_CHANGED:
      return {
        ...state,
        voxels: {
          ...state.voxels,
          [action.payload.position]: action.payload.filled,
        }
      }
    case BOT_CHANGED:
      return {
        ...state,
        bots: {
          ...state.bots,
          [action.payload.position]: action.payload.exists,
        }
      }
    case 'RESET':
      return initialState;
    default:
      return state
  }
}

export const changeSize = (newSize) => {
  return dispatch => {
    dispatch({
      type: SIZE_CHANGED,
      payload: {
        size: newSize,
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
        position: position.serialize ? position.serialize() : position,
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
        position: position.serialize ? position.serialize() : position,
      }
    })
  }
}
