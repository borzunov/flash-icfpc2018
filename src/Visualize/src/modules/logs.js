//import modelsDb from './indexedDb'

export const LOGS_UPDATED = 'logs/LOGS_UPDATED'

const initialState = {
  latest: [],
  loading: true,
}

export const tryGetLogs = (bd) => {
  return async (dispatch) => {
    // if (bd === 'models') {
    //     try {
    //       let ser = (await modelsDb.toArray());
    //       dispatch({
    //         type: LOGS_UPDATED,
    //         payload: ser.map(m => m.value),
    //         loading: false
    //       })
    //     }
    //     catch (e) {
    //       console.error(e)
    //       await doRefreshLogs(dispatch, bd)
    //     }
    //
    // } else {
    //
    // }
    await doRefreshLogs(dispatch, bd)
  }
}

async function doRefreshLogs(dispatch, bd) {

  let logs = []
  try {
    dispatch({
      type: LOGS_UPDATED,
      payload: [],
      loading: true
    })
    //debugger
    logs = await (await fetch(`http://vm-dev-cont4:3005/${bd}`)).json()
    // if (bd === 'models') {
    //   for (let m of logs) {
    //     await modelsDb.put({name: m.name, value: m})
    //   }
    // }
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

export const refreshLogs = (bd = 'logs') => {
  return async (dispatch) => {
    await doRefreshLogs(dispatch, bd)
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