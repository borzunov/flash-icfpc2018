export const LOGS_UPDATED = 'logs/LOGS_UPDATED'

const initialState = {
  latest: [],
  loading: true,
}

export const refreshLogs = () => {
  return async (dispatch) => {
    let logs = [];
    try {
      dispatch({
        type: LOGS_UPDATED,
        payload: [],
        loading: true,
      })
      logs = await (await fetch('http://vm-dev-cont4:3005/logs')).json()
    }
    catch(e) {
      console.error(e);
    }
    finally {
      dispatch({
        type: LOGS_UPDATED,
        payload: logs,
        loading: false,
      })
    }
  }
}

export default (state = initialState, action) => {
  switch (action.type) {
    case LOGS_UPDATED:
      return {
        ...state,
        latest: action.payload,
        loading: action.loading,
      }
    default:
      return initialState;
  }
};