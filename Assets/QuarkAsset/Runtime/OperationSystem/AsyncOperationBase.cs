﻿using System;
using System.Collections;
namespace Quark
{
    public abstract class AsyncOperationBase : IEnumerator, IComparable<AsyncOperationBase>
    {
        /// <summary>
        /// 是否已经完成
        /// </summary>
        internal bool IsFinish = false;
        /// <summary>
        /// 状态
        /// </summary>
        public AsyncOperationStatus Status { get; protected set; } = AsyncOperationStatus.None;
        /// <summary>
        /// 是否已经完成
        /// </summary>
        public bool IsDone
        {
            get
            {
                return Status == AsyncOperationStatus.Failed
                    || Status == AsyncOperationStatus.Succeeded;
            }
        }
        /// <summary>
        /// 优先级
        /// </summary>
        public uint Priority { set; get; } = 0;
        /// <summary>
        /// 处理进度
        /// </summary>
        public float Progress { get; protected set; }
        /// <summary>
        /// 错误信息
        /// </summary>
        public string Error { get; protected set; }
        Action<AsyncOperationBase> completed;
        public event Action<AsyncOperationBase> Completed
        {
            add
            {
                if (IsDone)
                    value.Invoke(this);
                else
                    completed += value;
            }
            remove { completed -= value; }
        }
        object IEnumerator.Current => null;
        public int CompareTo(AsyncOperationBase other)
        {
            return other.Priority.CompareTo(this.Priority);
        }
        bool IEnumerator.MoveNext()
        {
            return !IsDone;
        }
        void IEnumerator.Reset()
        {
        }
        internal abstract void OnStart();
        internal abstract void OnUpdate();
        internal abstract void OnAbort();
        internal void SetStart()
        {
            Status = AsyncOperationStatus.Processing;
            OnStart();
        }
        internal void SetFinish()
        {
            IsFinish = true;
            Progress = 1f;
            completed?.Invoke(this);
        }
        internal void SetAbort()
        {
            if (!IsDone)
            {
                Status = AsyncOperationStatus.Failed;
                Error = "abort";
                OnAbort();
            }
        }
        protected void ClearCompleteCallback()
        {
            completed = null;
        }
    }
}