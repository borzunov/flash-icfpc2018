import { LogAction } from '../../test-logs/LogAction'

export const playLog = ({ changeSize, changeVoxel, changeBot, changeColor, changeEnergy, changeHarmonic }, { size, log, name }, enqueue) => {
  changeSize(size)
  if (log.length > 5000) {
    console.error(`log is longer than 5000 and will crash app. Won't play`)
    return
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
      default:
        console.warn('bad act', act);
        break
    }
  }
}