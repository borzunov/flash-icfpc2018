import React from 'react'

export default function HelpText() {
  return <div style={{opacity: 0.7, textAlign: 'right', position: 'absolute', right: 20, top: 20, color: 'white', width: '50%'}}>
    <div>Time-travel: open in chrome and install <a target="_blank" rel="noopener noreferrer"
                                       href="https://chrome.google.com/webstore/detail/redux-devtools/lmhkpmbekcpmknklioeibfkpmmfibljd?hl=en">redux
      dev tools</a>, then choose the 'data' store and use timer button at the bottom</div>
    <div>Coordinates: <span style={{ color: 'red' }}>X </span><span style={{ color: 'green' }}>Y </span><span
      style={{ color: 'blue' }}>Z </span> <span>!!!Z is inverted in regard to task</span></div>
    <div>Left mouse - rotate camera, mouse scroll - zoom, right mouse - pan</div>
    <div>Arrow buttons - move camera, M - toggle menu</div>
  </div>
}