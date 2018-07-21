import { LogAction } from '../../test-logs/LogAction'

const MAX = 25000;
export const playLog = ({ changeSize, changeVoxel, changeBot, changeColor, changeEnergy, changeHarmonic, changeMessage }, { size, log, name }, enqueue) => {
  changeSize(size)
  if (log.length > MAX) {
    console.error(`log is longer (${log.length}) than ${MAX} and will crash app. Won't play`)
    return
  }
  else if (window.showLog) {
    console.log(JSON.stringify({size, log, name}))
  }
  for (let act of log) {
    switch (act.t) {
      case LogAction.Add:
        enqueue(() => changeBot(act.p, true))
        break
      case LogAction.Remove:
        enqueue(() => changeBot(act.p, false))
        break
      case LogAction.Fill:
        enqueue(() => changeVoxel(act.p, true))
        break
      case LogAction.FillColor:
        enqueue(() => changeColor(act.p, act.c, act.o))
        break
      case LogAction.Energy:
        enqueue(() => changeEnergy(act.e))
        break
      case LogAction.Harmonic:
        enqueue(() => changeHarmonic(act.h))
        break
      case LogAction.Message:
        enqueue(() => changeMessage(act.m))
        break
      default:
        console.warn('bad act', act);
        break
    }
  }
}