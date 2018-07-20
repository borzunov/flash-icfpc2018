export const SIZE_CHANGED = 'space/SIZE_CHANGED'

const initialState = {
  size: 10,
}

export default (state = initialState, action) => {
  switch (action.type) {
    case SIZE_CHANGED:
      return {
        ...state,
        size: action.payload.size,
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