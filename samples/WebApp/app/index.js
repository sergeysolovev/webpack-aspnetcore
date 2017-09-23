import './index.less';
import './images/banner1.svg';
import './images/banner2.svg';
import './images/banner3.svg';
import './images/banner4.svg';

const hasAnyClasses = (...args) => args.some(
  cls => document.getElementsByClassName(cls).length
);

window.addEventListener('load', function () {
  if (hasAnyClasses('carousel')) {
    import(/* webpackChunkName: 'carousel' */ './components/carousel')
      .catch(error => console.error(error));
  }
});
