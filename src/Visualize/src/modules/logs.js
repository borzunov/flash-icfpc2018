export const LOGS_UPDATED = 'logs/LOGS_UPDATED'
export const CURRENT_LOG_CHANGED = 'logs/CURRENT_LOG_CHANGED'

const mongoTopology = `http://vm-dev-cont4.dev.kontur.ru:3005`

const initialState = {
  latest: [],
  loading: true,
  currentLog: {
    loading: true,
    data: null,
  }
}

export const getLog = (bd, _id) => {
  return async (dispatch) => {
    let log;
    try {
      dispatch({
        type: CURRENT_LOG_CHANGED,
        payload: {
          loading: true,
          data: null
        }
      })
      log = await (await fetch(`${mongoTopology}/${bd}`)).json()[0]
    }
    catch(e) {
      console.error(e)
    }
    finally {
      dispatch({
        type: CURRENT_LOG_CHANGED,
        payload: {
          loading: false,
          data: log,
        },

      })
    }
  }
}

export const refreshLogs = (bd = 'logs') => {
  return async (dispatch) => {
    let logs = []
    try {
      dispatch({
        type: LOGS_UPDATED,
        payload: [],
        loading: true
      })
      logs = await (await fetch(`${mongoTopology}/${bd}`)).json()
    }
    catch (e) {
      console.error(e)
    }
    finally {
      dispatch({
        type: LOGS_UPDATED,
        payload: logs,
        loading: false
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
        loading: action.loading
      }
    default:
      return initialState
  }
};