import { combineReducers } from 'redux'
import space from './space'
import logs from './logs'

export default combineReducers({
  logs
})

const dataReducer = combineReducers({
  space,
})

export {dataReducer};
