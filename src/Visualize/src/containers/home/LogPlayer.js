import React from 'react'
import { connect } from 'react-redux'
import { refreshLogs } from '../../modules/logs'
import { bindActionCreators } from 'redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import { playLog } from './playLog'
import { withHandlers, compose } from 'recompose'
import Queue from 'async/queue'
import store from '../../store'
import Loader from 'react-loaders'
import 'loaders.css/loaders.min.css'

let currentWait = 10
const SPEED_CONST = 4

const reset = () => {
  // clear queue to avoid more actions that are not finished
  syncQueueWithWait.pause()
  syncQueueWithWait.remove(() => true)
  syncQueueWithWait.resume()
  // reset dev-tools
  store.liftedStore.dispatch({ type: 'RESET' })
  // reset our state
  store.dispatch({ type: 'RESET' })
}


const syncQueueWithWait = new Queue((f, callback) => {
  f()
  setTimeout(callback, currentWait)
}, 1)

class LogPlayer extends React.PureComponent {
  componentDidMount() {
    this.props.refreshLogs();
  }

  componentWillUnmount() {
  }

  render() {
    const { latest, playLog, loading, refreshLogs } = this.props

    function rednerFetched() {
      return latest.map((l, i) => <button onClick={() => playLog(l)} key={i}>{i + 1}: {l.name}</button>)
    }

    return <div className="log-player">
      <h3 onClick={refreshLogs} style={{color: 'white'}}>{loading ? 'Fetching logs...' : 'Click to play'}</h3>
      {loading ? <Loader style={{marginTop: 30}} type={'ball-scale-ripple-multiple'}/> : rednerFetched()}
    </div>
  }
}

export default compose(
  connect(
    ({ logs }) => {
      return {
        latest: logs.latest.slice(0, 10),
        loading: logs.loading,
      }
    },
    dispatch => {
      return bindActionCreators({
        refreshLogs,
        changeSize,
        changeVoxel,
        changeBot
      }, dispatch)
    }
  ),
  withHandlers({
    playLog: ({changeSize, changeBot, changeVoxel}) => ({log, size}) => {
      reset();
      playLog({changeSize, changeBot, changeVoxel}, {size, log}, (t, cb) => syncQueueWithWait.push(t, cb));
    }
  })
)(LogPlayer)