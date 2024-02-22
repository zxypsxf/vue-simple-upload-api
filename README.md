Vue+ElementUI+Asp.net Core实现的分片上传

前端组件参考了
[vue-simple-upload](https://github.com/debug-null/vue-simple-upload)

这里主要是实现.net后端接口：
1、检查文件是否存在api/fileChunk/upload，本项目该接口没有具体的功能，直接返回了false，实际项目中可以用数据库保存已上传文件列表，从数据库中查询文件状态并返回结果
2、分片上传api/fileChunk/upload
3、分片合并api/fileChunk/merge
