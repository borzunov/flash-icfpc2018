import { LogAction } from './LogAction'


const log = []

const makeLog = (pos, type) => ({p: pos, t: type})

log.push(makeLog('0/1/0', LogAction.Add));
for (let i = 0; i < 10; ++i) {
  log.push(makeLog(`${i}/0/0`, LogAction.Fill));
  log.push(makeLog(`${i}/1/0`, LogAction.Remove));
  if (i !== 9)
    log.push(makeLog(`${i + 1}/1/0`, LogAction.Add));
}
export default {
  size: 10,
  log
}