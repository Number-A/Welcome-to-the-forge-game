using System;

public class PriorityQueue<T>
{
    private Node m_head;
    private Node m_dummy;

    public PriorityQueue()
    {
        m_dummy = new Node();
        m_head = m_dummy;
    }

    //lower number means higher priority
    public void Enqueue(T element, int priority = 0)
    {
        Node n = new Node(element, priority);

        if (IsEmpty())
        {
            m_head = n;
            n.SetNext(m_dummy);
            m_dummy.SetPrev(m_head);
            return;
        }

        int priorityDiffAtHead = Math.Abs(m_head.GetPriority() - priority);

        if (priorityDiffAtHead > Math.Abs(m_dummy.GetPrev().GetPriority() - priority))
        {
            InsertNodeFromBack(n);
        }
        else
        {
            InsertNodeFromFront(n);
        }
    }

    public T Dequeue()
    {
        if (IsEmpty())
        {
            throw new EmptyQueueException();
        }

        Node dequeued = m_dummy.GetPrev();

        RemoveNode(dequeued);
        return dequeued.GetElement();
    }

    public bool IsEmpty() { return m_dummy == m_head; }

    public void SetPriority(T element, int priority)
    {
        Node n = FindNode(element);

        if (n != null)
        {
            RemoveNode(n);
        }
        Enqueue(element, priority);
    }

    public int GetPriority(T element)
    {
        Node n = FindNode(element);

        if (n == null)
        {
            throw new ArgumentException("Could not find the element provided in the PriorityQueue");
        }

        return n.GetPriority();
    }

    public bool Contains(T element)
    {
        return FindNode(element) != null;
    }

    //helper functions///////////////////////////////////////////////////////

    //returns null if not found
    private Node FindNode(T element)
    {
        Node ptr = m_head;

        while (ptr != m_dummy)
        {
            if (ptr.GetElement().Equals(element))
            {
                return ptr;
            }
            ptr = ptr.GetNext();
        }
        return null;
    }

    private void InsertNodeFromBack(Node n)
    {
        Node ptr = m_dummy;

        while (ptr != m_head && n.GetPriority() >= ptr.GetPrev().GetPriority())
        {
            ptr = ptr.GetPrev();
        }

        InsertNodeAtPos(n, ptr);
    }

    private void InsertNodeFromFront(Node n)
    {
        Node ptr = m_head;

        while (n.GetPriority() < ptr.GetPriority() && ptr != m_dummy)
        {
            ptr = ptr.GetNext();
        }

        InsertNodeAtPos(n, ptr);
    }

    private void InsertNodeAtPos(Node toInsert, Node pos)
    {
        toInsert.SetNext(pos);
        toInsert.SetPrev(pos.GetPrev());

        if (pos.GetPrev() != null)
        {
            pos.GetPrev().SetNext(toInsert);
        }
        pos.SetPrev(toInsert);

        if (pos == m_head)
        {
            m_head = toInsert;
        }
    }

    private void RemoveNode(Node n)
    {
        if (n.GetPrev() != null)
        {
            n.GetPrev().SetNext(n.GetNext());
        }

        n.GetNext().SetPrev(n.GetPrev());

        if (m_head == n)
        {
            m_head = n.GetNext();
        }
    }

    private class Node
    {
        int m_priority;
        T m_element;
        Node m_next;
        Node m_prev;

        public Node() { }

        public Node(T element, int priority)
        {
            m_priority = priority;
            m_element = element;
        }

        public T GetElement() { return m_element; }

        public int GetPriority() { return m_priority; }

        public Node GetNext() { return m_next; }

        public Node GetPrev() { return m_prev; }

        public void SetNext(Node n) { m_next = n; }

        public void SetPrev(Node n) { m_prev = n; }
    }
}

public class EmptyQueueException : Exception
{
    public EmptyQueueException() : base("Operation cannot be performed on an empty PriorityQueue") { }
    public EmptyQueueException(string message) : base(message) { }
}
