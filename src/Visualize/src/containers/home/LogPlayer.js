import React from 'react'
import { connect } from 'react-redux'
import { refreshLogs } from '../../modules/logs'
import { bindActionCreators } from 'redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import { playLog } from './playLog'
import { withHandlers, compose, withProps } from 'recompose'
import Queue from 'async/queue'
import store, { dataStore } from '../../store'
import Loader from 'react-loaders'
import 'loaders.css/loaders.min.css'

let currentWait = 10

class LogPlayer extends React.PureComponent {
  componentDidMount() {
    this.props.refreshLogs()
    this.syncQueueWithWait = new Queue((f, callback) => {
      f()
      setTimeout(callback, currentWait)
    }, 1)
  }

  componentWillUnmount() {
    this.syncQueueWithWait.kill()
  }

  reset = () => {
    // clear queue to avoid more actions that are not finished
    this.syncQueueWithWait.pause()
    this.syncQueueWithWait.remove(() => true)
    this.syncQueueWithWait.resume()
    // reset dev-tools
    if (dataStore.liftedStore)
      dataStore.liftedStore.dispatch({ type: 'RESET' })
    // reset our state
    dataStore.dispatch({ type: 'RESET' })
  }

  render() {
    const { latest, playLog, loading, refreshLogs } = this.props

    const rednerFetched = () => {
      return latest.map((l, i) => <button onClick={() => playLog(l, this.reset, this.syncQueueWithWait.push)}
                                          key={i}>{i + 1}: {l.name}</button>)
    }

    return <div className="log-player">
      <h3 onClick={refreshLogs} style={{ color: 'white' }}>{loading ? 'Fetching logs...' : 'Click to play'}</h3>
      {loading ? <Loader style={{ marginTop: 30 }} type={'ball-scale-ripple-multiple'}/> : rednerFetched()}
    </div>
  }
}

export default compose(
  withProps({
    ...bindActionCreators({
      changeSize,
      changeVoxel,
      changeBot
    }, dataStore.dispatch)
  }),
  connect(
    ({ logs }) => {
      return {
        latest: logs.latest.slice(0, 10),
        loading: logs.loading
      }
    },
    dispatch => bindActionCreators({
      refreshLogs
    }, dispatch)
  ),
  withHandlers({
    playLog: ({ changeSize, changeBot, changeVoxel }) => ({ log, size }, reset, push) => {
      reset()
      playLog({ changeSize, changeBot, changeVoxel }, { size, log }, push)
    }
  })
)
(LogPlayer)