export const LOGS_UPDATED = 'logs/LOGS_UPDATED'

const initialState = {
  latest: []
}

export const refreshLogs = () => {
  return async (dispatch) => {
    try {
      const logs = await (await fetch('http://localhost:3005/logs')).json()
      dispatch({
        type: LOGS_UPDATED,
        payload: logs,
      })
    }
    catch(e) {
      console.error(e);
    }
  }
}

export default (state = initialState, action) => {
  switch (action.type) {
    case LOGS_UPDATED:
      return {
        ...state,
        latest: action.payload,
      }
    default:
      return initialState;
  }
};