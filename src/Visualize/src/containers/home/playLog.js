import { LogAction } from '../../test-logs/LogAction'

export const playLog = ({ changeSize, changeVoxel, changeBot, changeColor }, { size, log }, enqueue) => {
  changeSize(size)
  if (log.length > 5000)
  {
    console.error(`log is longer than 5000 and will crash app. Won't play`);
    return;
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
      default:
        // do nothing
        break
    }
  }
}