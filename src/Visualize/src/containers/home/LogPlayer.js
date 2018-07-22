import React from 'react'
import { connect } from 'react-redux'
import { refreshLogs } from '../../modules/logs'
import { bindActionCreators } from 'redux'
import {
  changeBot,
  changeSize,
  changeVoxel,
  changeColor,
  changeHarmonic,
  changeEnergy,
  changeMessage,
  changeVoxelBatch, changeColorBatch
} from '../../modules/space'
import { playLog } from './playLog'
import { withHandlers, compose, withProps, mapProps } from 'recompose'
import Queue from 'async/queue'
import { dataStore } from '../../store'
import Loader from 'react-loaders'
import 'loaders.css/loaders.min.css'
import case1 from '../../test-logs/1'
import case2 from '../../test-logs/2'
import case3 from '../../test-logs/3'
// import case4 from '../../test-logs/4'
// import case5 from '../../test-logs/5'
// import case6 from '../../test-logs/6'

let currentWait = 10

let passed = 0
let total = 0

class LogPlayer extends React.PureComponent {
  componentDidMount() {
    this.props.refreshLogs();
    this.syncQueueWithWait = new Queue((f, callback) => {
      try {
        f()
      }
      catch (e) {
        console.error(e)
      }
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
    this.forceUpdate()
  }

  bindFocus() {
    if (this.logList && !this.bound) {
      this.bound = true
      this.logList.addEventListener('mouseenter', () => {
        this.logList.focus()
        console.log('zoom disabled')
        window.controls.enableZoom = false
      })
      this.logList.addEventListener('mouseleave', () => {
        console.log('zoom enabled')
        window.controls.enableZoom = true
      })
    }
  }

  componentDidUpdate() {
    this.bindFocus()
  }

  render() {
    let { latest, playLog, loading, refreshLogs, bd } = this.props
    if (bd === 'logs') {
      latest = latest.concat([case1, case2, case3]);
      //latest = latest.concat([case4, case5, case6])
    }
    if (bd === 'models')
      latest = latest.concat([]) // TODO
    const queueLength = this.syncQueueWithWait ? this.syncQueueWithWait.length() : 0
    let content
    if (loading)
      content = <div className="flex-column">
        <h3>Fetching {bd}...</h3>
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
        <div ref={(logList) => this.logList = logList}
             style={{ overflow: 'auto', minHeight: 0, WebkitFlex: '1 1 auto' }}>
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
      changeBot,
      changeColor,
      changeHarmonic,
      changeEnergy,
      changeMessage,
      changeVoxelBatch,
      changeColorBatch,
    }, dataStore.dispatch)
  }),
  connect(
    ({ logs }, { limit = 999999 }) => {
      return {
        latest: logs.latest.slice(0, limit),
        loading: logs.loading
      }
    },
    dispatch => bindActionCreators({
      refreshLogs,
    }, dispatch)
  ),
  withHandlers({
    playLog: ({ changeSize, changeBot, changeVoxel, changeColor, changeEnergy, changeHarmonic, changeMessage, changeVoxelBatch, changeColorBatch, }) => ({ log, size }, reset, push) => {
      reset()
      passed = 0
      total = log.length
      playLog({
        changeSize,
        changeBot,
        changeVoxel,
        changeColor,
        changeHarmonic,
        changeEnergy,
        changeMessage,
        changeVoxelBatch,
        changeColorBatch,
      }, {
        size,
        log
      }, push)
    }
  }),
  mapProps(({ refreshLogs, bd, ...rest }) => ({
    refreshLogs: () => refreshLogs(bd),
    bd,
    ...rest
  }))
)(LogPlayer)