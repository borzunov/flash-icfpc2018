import React from 'react'
import { connect } from 'react-redux'
import { refreshLogs } from '../../modules/logs'
import { bindActionCreators } from 'redux'
import { changeBot, changeSize, changeVoxel } from '../../modules/space'
import { playLog } from './playLog'
import { withHandlers, compose, withProps } from 'recompose'
import Queue from 'async/queue'
import { dataStore } from '../../store'
import Loader from 'react-loaders'
import 'loaders.css/loaders.min.css'

let currentWait = 10

let passed = 0
let total = 0

class LogPlayer extends React.PureComponent {
  componentDidMount() {
    this.props.refreshLogs()
    this.syncQueueWithWait = new Queue((f, callback) => {
      f()
      passed++
      this.forceUpdate()
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

    const queueLength = this.syncQueueWithWait ? this.syncQueueWithWait.length() : 0
    let content
    if (loading)
      content = <div className="flex-column">
        <h3>Fetching logs...</h3>
        <Loader style={{ marginTop: 30 }} type={'ball-scale-ripple-multiple'}/>
      </div>
    else if (queueLength > 0)
      content = <div className="flex-column" onClick={this.reset}>
        <h3>Playing: {passed}/{total}</h3>
        <h4>Click to stop</h4>
        <Loader onClick={this.reset} style={{ marginTop: 30 }} type={'pacman'}/>
      </div>
    else
      content = <div className="flex-column">
        <h3 onClick={refreshLogs}>Click here to refresh</h3>
        <h4>Click any button to play</h4>
        <div>
          {latest.map((l, i) => <button onClick={() => playLog(l, this.reset, this.syncQueueWithWait.push)}
                                        key={i}>{i + 1}: {l.name}</button>)}
        </div>
      </div>

    return <div className="log-player">
      {content}
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
      passed = 0
      total = log.length
      playLog({ changeSize, changeBot, changeVoxel }, { size, log }, push)
    }
  })
)(LogPlayer)