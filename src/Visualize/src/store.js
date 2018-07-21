import { createStore, applyMiddleware, compose } from 'redux'
import { connectRouter, routerMiddleware } from 'connected-react-router'
import thunk from 'redux-thunk'
import createHistory from 'history/createBrowserHistory'
import rootReducer, { dataReducer } from './modules'

export const history = createHistory()

const initialState = {}
const enhancers = []
const dataEnhancers = []
const middleware = [thunk, routerMiddleware(history)]

const devToolsExtension = window.__REDUX_DEVTOOLS_EXTENSION__

if (typeof devToolsExtension === 'function') {
  enhancers.push(devToolsExtension({
    name: 'main'
  }))
  dataEnhancers.push(devToolsExtension({
    maxAge: 10000,
    name: 'data'
  }))
}

const composedEnhancersMain = compose(
  applyMiddleware(...middleware),
  ...enhancers
)

const composedEnhancersData = compose(
  applyMiddleware(...middleware),
  ...dataEnhancers
)

export default createStore(
  connectRouter(history)(rootReducer),
  initialState,
  composedEnhancersMain,
)

const dataStore = createStore(
  dataReducer,
  initialState,
  composedEnhancersData,
)

export {dataStore}
