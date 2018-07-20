export const SIZE_CHANGED = 'space/SIZE_CHANGED'
export const VOXEL_CHANGED = 'space/VOXEL_CHANGED'

const initialState = {
  size: 10,
  voxels: {
  }
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
        position: position.serialize(),
      }
    })
  }
}

// export const incrementAsync = () => {
//   return dispatch => {
//     dispatch({
//       type: INCREMENT_REQUESTED
//     })
//
//     return setTimeout(() => {
//       dispatch({
//         type: INCREMENT
//       })
//     }, 3000)
//   }
// }