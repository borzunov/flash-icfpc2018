import React from 'react'
import { connect } from 'react-redux'
import { refreshLogs } from '../../modules/logs'
import { bindActionCreators } from 'redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import { playLog } from './playLog'
import { withHandlers } from 'recompose'
import compose from 'redux/src/compose'
import Queue from 'async/queue'
import store from '../../store'

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
    this.refresher = window.setInterval(() => this.props.refreshLogs(), 5000)
  }

  componentWillUnmount() {
    clearInterval(this.refresher)
  }

  render() {
    const { latest, playLog } = this.props
    return <div className="log-player">
      <h3>Click on log to play</h3>
      {latest.map((l, i) => <button onClick={() => playLog(l)} key={i}>{i + 1}: {l.name}</button>)}
    </div>
  }
}

export default compose(
  connect(
    ({ logs }) => {
      return { latest: logs.latest.slice(0, 10) }
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