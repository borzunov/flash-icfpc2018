import { LogAction } from './LogAction'


const log = []

const makeLog = (pos, type, more) => ({p: pos, t: type, ...more})

log.push(makeLog('0/1/0', LogAction.Add));
for (let i = 0; i < 10; ++i) {
  log.push(makeLog(`${i}/0/0`, LogAction.Fill));
  log.push(makeLog(`${i}/1/0`, LogAction.Remove));
  if (i !== 9) {
    log.push(makeLog(`${i + 1}/1/0`, LogAction.Add));
    log.push(makeLog(`${i + 1}/3/0`, LogAction.FillColor, {c: 'ff0000', o: i / 10}));
  }
}
export default {
  size: 10,
  log,
  name: 'static test case 1'
}