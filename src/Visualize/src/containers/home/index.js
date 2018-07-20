//import { push } from 'connected-react-router'
import { bindActionCreators } from 'redux'
import { connect } from 'react-redux'
import ThreeScene from './ThreeScene'
import { changeSize, changeVoxel, changeBot } from '../../modules/space'

const mapStateToProps = ({ space }) => ({
  mapSize: space.size,
})

const mapDispatchToProps = dispatch =>
  bindActionCreators(
    {
      changeSize,
      changeVoxel,
      changeBot,
    },
    dispatch
  )

export default connect(
  mapStateToProps,
  mapDispatchToProps
)(ThreeScene)
