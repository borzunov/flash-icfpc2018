import { LogAction } from '../../test-logs/LogAction'

export const playLog = ({ changeSize, changeVoxel, changeBot }, { size, log }, enqueue) => {
  changeSize(size)
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
      default:
        // do nothing
        break
    }
  }
}